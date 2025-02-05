import { CanActivate, ExecutionContext, Injectable } from '@nestjs/common';
import { JwtService } from '@nestjs/jwt';
import { Request } from 'express';
import { env, IS_PUBLIC_ROUTE_KEY } from '../constants';
import { UnauthorizedException } from '../exceptions';
import { Reflector } from '@nestjs/core';
import { PrismaService } from '~prisma/module';
import { createDecipheriv } from 'crypto';
@Injectable()
export class AuthGuard implements CanActivate {
  constructor(
    private readonly jwtService: JwtService,
    private readonly reflector: Reflector,
    private readonly prisma: PrismaService,
  ) {}

  /* async canActivate(context: ExecutionContext): Promise<boolean> {
    const isPublic = this.reflector.getAllAndOverride<boolean>(
      IS_PUBLIC_ROUTE_KEY,
      [context.getHandler(), context.getClass()],
    );

    if (isPublic) {
      return true;
    }

    const request = context.switchToHttp().getRequest();
    const token = this.extractTokenFromHeader(request);
    if (!token) {
      throw new UnauthorizedException();
    }
    try {
      const payload = await this.jwtService.verifyAsync(token, {
        secret: env.JWT_SECRET,
      });

      const credentials = await this.prisma.credentials.findFirst({
        where: {
          id: payload.id,
        },
        include: {
          collaborator: true,
        },
      });

      request['collaborator'] = credentials.collaborator;
      request['credential'] = credentials;

      return true;
    } catch {
      throw new UnauthorizedException();
    }
  } */
    async canActivate(context: ExecutionContext): Promise<boolean> {

      const isPublic = this.reflector.getAllAndOverride<boolean>(
        IS_PUBLIC_ROUTE_KEY,
        [context.getHandler(), context.getClass()],
      );
  
      if (isPublic) {
        return true;
      }
  
      const request = context.switchToHttp().getRequest();
      const token = this.extractTokenFromHeader(request);
      if (!token) {
        throw new UnauthorizedException();
      }

      try {
        const payload = await this.jwtService.verifyAsync(token, {
          secret: env.JWT_SECRET,
        });
  
        // Descriptografar os campos sensíveis
        const key = Buffer.from(env.JWT_SECRET, 'ascii');
        const iv = Buffer.from(payload.IV, 'base64'); // Recupera o IV do token

        payload.id = this.decryptAES(payload.id, key, iv);
        payload.collaboratorId = this.decryptAES(payload.collaboratorId, key, iv);
        payload.codLog = this.decryptAES(payload.codLog, key, iv);
        payload.personaId = this.decryptAES(payload.personaId, key, iv);

        const credentials = await this.prisma.credentials.findFirst({
          where: {
            id: parseInt(payload.id, 10),
          },
          include: {
            collaborator: true,
          },
        });

        request['collaborator'] = credentials.collaborator;
        request['credential'] = credentials;
  
        return true;
      } catch (error) {
        console.error(error);
        throw new UnauthorizedException();
      }
    }
  
    private extractTokenFromHeader(request: Request): string | undefined {
      const [type, token] = request.headers.authorization?.split(' ') ?? [];
      return type === 'Bearer' ? token : undefined;
    }
  
    private decryptAES(encryptedText: string, key: Buffer, iv: Buffer): string {
      const decipher = createDecipheriv('aes-256-cbc', key, iv);
      let decrypted = decipher.update(encryptedText, 'base64', 'utf8');
      decrypted += decipher.final('utf8');
      return decrypted;
    }
    
/*   private extractTokenFromHeader(request: Request): string | undefined {
    const [type, token] = request.headers.authorization?.split(' ') ?? [];
    return type === 'Bearer' ? token : undefined;
  } */
}
