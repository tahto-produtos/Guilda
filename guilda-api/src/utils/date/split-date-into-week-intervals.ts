import { addDays } from 'date-fns';

export function splitDateIntoWeekIntervals(startDate: Date, endDate: Date) {
  const parts: { start: Date; end: Date }[] = [];

  let currentStart = startDate;

  while (currentStart <= endDate) {
    let currentEnd = addDays(currentStart, 6);
    if (currentEnd > endDate) {
      currentEnd = endDate;
    }

    parts.push({ start: new Date(currentStart), end: new Date(currentEnd) });

    currentStart = addDays(currentEnd, 1);
  }

  return parts;
}
