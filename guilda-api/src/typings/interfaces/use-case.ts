export interface UseCase<Input = unknown, Output = unknown> {
  handle(input: Input, ...args: unknown[]): Promise<Output>;
}
