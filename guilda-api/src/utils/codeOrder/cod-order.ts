export class CodeOrder {
  static generate() {
    const caracteres = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ';
    return Array.from(
      { length: 7 },
      () => caracteres[Math.floor(Math.random() * caracteres.length)],
    ).join('');
  }
}
