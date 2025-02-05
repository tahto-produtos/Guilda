import { PrismaClient } from '@prisma/client';

export abstract class Seeder<T = any> {
  public constructor(public readonly prisma: PrismaClient) {}

  abstract seed(): Promise<void>;
  abstract get data(): T;
}
