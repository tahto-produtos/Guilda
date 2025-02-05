import { format, endOfDay, formatISO, isValid } from "date-fns";

export class DateUtils {
  static formatDatePtBR(date: Date | string) {
    return format(new Date(date), "dd/MM/yyyy");
  }

  static formatDatePtBRWithTime(date: Date | string) {
    return format(new Date(date), "dd/MM/yyyy kk:mm");
  }


  static formatDatePtBRWithTimeWithoutDay(date: Date | string) {
    return format(new Date(date), "HH:mm");
  }

  static formatDateIsoEndOfDay(date: Date | dateFns | string){
    return formatISO(new Date(endOfDay(new Date(date.toString()))))
  }
  
  static isValidDate(date: Date) {
    return isValid(date);
  }

  static formatDatePtBRWithTimeAndSecondsToUS(date: string) {
    if (!date || typeof date !== 'string') {
        throw new Error("A data fornecida é inválida ou não é uma string.");
    }

    // Divide a data e a hora
    const [data, hora] = date.split(' ');

    if (!data || !hora) {
        throw new Error("Formato de data inválido. Certifique-se de usar 'dd/MM/yyyy HH:mm:ss'.");
    }

    // Divide a parte da data
    const [dia, mes, ano] = data.split('/');

    if (!dia || !mes || !ano) {
        throw new Error("A parte da data não segue o formato 'dd/MM/yyyy'.");
    }

    // Monta a data no formato americano
    const dataUs = `${ano}-${mes}-${dia}T${hora}`;

    return dataUs;
}

  static formatDateTimePtBR(date: string,) {
    const [data, hora] = date.split('T');
    const [ano, mes, dia] = data.split('-');
    const dataUs = `${dia}/${mes}/${ano} ${hora}`;
    
    return dataUs.replaceAll(',', '');
  }


}
