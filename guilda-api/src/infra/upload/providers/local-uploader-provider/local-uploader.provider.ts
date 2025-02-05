import { Injectable, NotFoundException } from '@nestjs/common';
import { accessSync, createReadStream, unlinkSync } from 'fs';

import { writeFile, access, mkdir } from 'node:fs/promises';

import * as uuid from 'uuid';
import * as mime from 'mime';
import { join } from 'path';
import { FileUploaderStrategy } from '../../../../typings/interfaces/file-uploader-strategy';
import { env } from '../../../../constants';
import { FileMetadata } from '../../../../typings/interfaces/file-metadata';

@Injectable()
export class LocalUploadProvider implements FileUploaderStrategy {
  private readonly path: string;

  constructor() {
    this.path = join(process.cwd(), 'uploads');
  }

  private getFilePath(key: string) {
    return join(this.path, key);
  }

  private static getFileUrl(key: string) {
    const protocol = env.API_PROTOCOL;
    const host = env.API_HOST;
    const getFileEndpoint = 'upload';
    return `${protocol}://${host}/${getFileEndpoint}/${key}`;
  }

  async exists(key: string): Promise<boolean> {
    try {
      accessSync(this.getFilePath(key));
      return true;
    } catch {
      return false;
    }
  }

  async generateKey(file: Express.Multer.File): Promise<string> {
    const id = uuid.v4();
    const type = mime.getExtension(file.mimetype);
    const key = `${id}.${type}`;
    const exists = await this.exists(key);
    if (exists) {
      return this.generateKey(file);
    }
    return key;
  }

  async upload(file: Express.Multer.File): Promise<FileMetadata> {
    return new Promise(async (resolve, reject) => {
      const key = await this.generateKey(file);

      try {
        await access(this.path);
      } catch (e) {
        await mkdir(this.path);
      }

      try {
        await writeFile(this.getFilePath(key), file.buffer);
        const fileMetadata = new FileMetadata();
        fileMetadata.key = key;
        fileMetadata.originalName = file.originalname;
        fileMetadata.mimeType = file.mimetype;
        fileMetadata.url = LocalUploadProvider.getFileUrl(key);

        resolve(fileMetadata);
      } catch (err) {
        unlinkSync(this.getFilePath(key));
        reject(err);
      }
    });
  }

  async getFile(key: string) {
    const exists = await this.exists(key);
    if (!exists) {
      throw new NotFoundException(`cannot find upload with key: ${key}`);
    }
    return createReadStream(this.getFilePath(key));
  }

  async delete(key: string) {
    try {
      const exists = await this.exists(key);
      if (exists) {
        unlinkSync(this.getFilePath(key));
      }
      return true;
    } catch {
      throw new NotFoundException(`cannot find upload with key: ${key}`);
    }
  }
}
