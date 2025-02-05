import { Express } from 'express';

import { Readable } from 'stream';
import { FileMetadata } from './file-metadata';

export abstract class FileUploaderStrategy {
  abstract generateKey(file: Express.Multer.File): Promise<string>;
  abstract upload(file: Express.Multer.File): Promise<FileMetadata>;
  abstract exists(key: string): Promise<boolean>;
  abstract delete(key: string): Promise<boolean>;
  abstract getFile(key: string): Promise<Readable>;
}
