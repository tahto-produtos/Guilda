import { INestApplication, Injectable, OnModuleInit } from '@nestjs/common';
import { PrismaClient } from '@prisma/client';
import { DATABASE_CONNECTION } from 'src/constants/environment-variable.constants';
import * as os from 'os';

@Injectable()
export class PrismaService extends PrismaClient implements OnModuleInit {
  constructor() {
    super({
      datasources: {
        db: {
          url: DATABASE_CONNECTION,
        },
      },
    });

    // Obter o nome do usuário logado na máquina
    const loggedInUser = os.userInfo().username;

    // Exibir informações no console
    console.log('Usuário logado na máquina:', loggedInUser);
    console.log('URL BANCO:', DATABASE_CONNECTION);
  }

  async onModuleInit() {
    try {
      await this.$connect();
      console.log('Conexão com o banco de dados estabelecida com sucesso.');
    } catch (error) {
      console.error('Erro ao conectar ao banco de dados:', error);
    }
  }

  async enableShutdownHooks(app: INestApplication) {
    this.$on('beforeExit', async () => {
      await app.close();
    });
  }
}
