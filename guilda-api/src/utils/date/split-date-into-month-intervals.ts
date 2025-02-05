import { startOfMonth, addMonths, isAfter, isSameMonth } from 'date-fns';

export function splitDateIntoMonthIntervals(
  startDate: Date,
  endDate: Date,
): Date[] {
  const months: Date[] = [];
  let currentMonth = startOfMonth(startDate);

  while (isBeforeOrEqual(currentMonth, endDate)) {
    months.push(currentMonth);
    currentMonth = addMonths(currentMonth, 1);
  }

  return months;
}

function isBeforeOrEqual(date: Date, compareTo: Date): boolean {
  return !isAfter(date, compareTo) || isSameMonth(date, compareTo);
}
