export function formatStringFirstLetterUppercase(input: string): string {
    const words = input.split(" ");
    const formattedWords = words.map((word) => {
        if (word.length > 0) {
            return word[0].toUpperCase() + word.slice(1).toLowerCase();
        }
        return "";
    });
    return formattedWords.join(" ");
}
