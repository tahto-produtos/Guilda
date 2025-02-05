export function translateVisibiltyName(name: string) {
    switch (name) {
        case "HIERARCHY":
            return "Hierarquia";
        case "COLLABORATOR":
            return "Colaborador";
        case "GROUP":
            return "Grupo";
        case "SECTOR":
            return "Setor";
        case "CLIENT":
            return "Cliente";
        default:
            return name;
    }
}
