import * as bcrypt from 'bcrypt';

export class Hashing {
  static async hash(str: string) {
    return bcrypt.hash(str, await bcrypt.genSalt(8));
  }

  static async compare(str: string, encrypted: string) {
    return bcrypt.compare(str, encrypted);
  }

  static hashSync(str: string) {
    return bcrypt.hashSync(str, bcrypt.genSaltSync(8));
  }

  static compareSync(str: string, encrypted: string) {
    return bcrypt.compareSync(str, encrypted);
  }
}
