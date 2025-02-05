import { Type } from '@nestjs/common';
import { PrismaClient } from '@prisma/client';
import { Seeder } from './seeder';

export class SeederRunner {
  constructor(private readonly prisma: PrismaClient) {}

  async run(...seeders: Type<Seeder<unknown>>[]) {
    for (const Seeder of seeders) {
      const seederInstance = new Seeder(this.prisma);

      console.log(`\nExecuting ${Seeder.name}`);
      await seederInstance.seed();
      console.log(`${Seeder.name} executed successfully`);
    }
  }
}
