import { NestFactory, Reflector } from '@nestjs/core';
import { DocumentBuilder, SwaggerModule } from '@nestjs/swagger';
import { JwtService } from '@nestjs/jwt';

import { PrismaService } from '~prisma/module';
import { AppModule } from './app.module';
import { env } from './constants';
import { InvalidEntriesException } from './exceptions';
import { AuthGuard, PermissionGuard } from './guards';
import { CustomValidationPipe } from './pipes';
import { GlobalExceptionFilter } from './exception-filters';
import helmet from 'helmet';

async function bootstrap() {
  const app = await NestFactory.create(AppModule);

  app.setGlobalPrefix(env.URL_API);

  app.enableCors({
    // todo - add prodution URL
    origin: process.env.NODE_ENV === 'production' ? '' : '*',
  });

  app.useGlobalFilters(new GlobalExceptionFilter());

  app.use(helmet());
  
  app.use((req, res, next) => {
    res.removeHeader('X-Powered-By');
    next();
  });

  const sanitizeObject = (obj) => {
    if (obj && typeof obj === 'object') {
      if ('__proto__' in obj) {
        delete obj.__proto__;
      }
      Object.keys(obj).forEach(key => {
        if (typeof obj[key] === 'object') {
          sanitizeObject(obj[key]);
        }
      });
    }
    return obj;
  };

  app.use((req, res, next) => {
    req.body = sanitizeObject(req.body);
    next();
  });

  app.useGlobalPipes(
    new CustomValidationPipe({
      exceptionFactory: (errors) => new InvalidEntriesException(errors),
      stopAtFirstError: true,
      transform: true,
    }),
  );
  const prismaService = app.get(PrismaService);
  await prismaService.enableShutdownHooks(app);
  const jwtService = app.get(JwtService);
  app.useGlobalGuards(
    new AuthGuard(jwtService, new Reflector(), prismaService),
  );
  /* app.useGlobalGuards(
    new PermissionGuard(new PrismaService(), new Reflector()),
  ); */

  const swaggerDocumentConfig = new DocumentBuilder()
    .setTitle('GUILDA API')
    .setDescription('GUILDA API Reference')
    .setVersion(env.APP_VERSION)
    .build();

  const swaggerDocument = SwaggerModule.createDocument(
    app,
    swaggerDocumentConfig,
  );

  SwaggerModule.setup('api', app, swaggerDocument, {
    swaggerOptions: {
      supportedSubmitMethods: [],
    },
  });

  await app.listen(env.API_PORT || 3001);
}

bootstrap();
