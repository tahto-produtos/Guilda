import defineAbilityFor from "../permissions/definitions/definitions.ability";

export default function abilityFor(myPermissions : number[]) {
    return defineAbilityFor(myPermissions)
}