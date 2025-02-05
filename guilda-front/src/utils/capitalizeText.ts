export function capitalizeText(name: string): string {
  if (!name) return name;

  const exceptions = ["II", "I"]; // Lista de palavras que não queres modificar
  const words = name.split(" ");

  const capitalizedWords = words.map(word => {
    if (exceptions.includes(word.toUpperCase())) {
      return word; // Retorna a palavra sem alteração se estiver nas exceções
    }
    word = word.toLowerCase();
    return word.charAt(0).toUpperCase() + word.slice(1); // Aplica capitalização normal
  });

  const capitalizedString = capitalizedWords.join(" ");

  return capitalizedString;
}
