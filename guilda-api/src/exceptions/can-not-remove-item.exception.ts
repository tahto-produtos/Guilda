import { HttpStatus } from '@nestjs/common';
import { BaseException, EXCEPTION_CODES } from '../typings';

export class CanNotRemoveItemsException extends BaseException {
  constructor(quantity: number) {
    super(
      {
        message: `It is not possible to remove the quantity of items in the product. There is ${quantity} registered items.`,
        code: EXCEPTION_CODES.CAN_NOT_REMOVE_ITEM,
        keys: {
          quantity,
        },
      },
      HttpStatus.BAD_REQUEST,
    );
  }
}
