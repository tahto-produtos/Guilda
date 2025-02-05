export const errorLabels = {
    REQUIRED_FIELD: (fieldName: string) =>
        `${fieldName} é um campo obrigatório`,
    VALID_EMAIL: () => `Insira um endereço de e-mail válido`,
    INTEGER_ONLY: (fieldName: string) =>
        `${fieldName} não pode conter pontuação`,
    ALMOST_ONE_ITEM: () => `Selecione pelo menos um item da lista`,
};
