import { guildaApiClient } from "src/services";
import { HomeFloor, Period } from "src/typings";
import { Site } from "src/typings/models/site.model";

export interface CollaboratorHierarchyProps {
    userId: number;
    startDate: string;
    endDate: string;
    sectorId: number;
    period: Period[];
    homeFloor: HomeFloor[];
    site: Site[];
    sectorIds?: number[];
}

export class CollaboratorHierarchy {
    private client = guildaApiClient;

    async handle(props: CollaboratorHierarchyProps) {
        const { userId, sectorId, startDate, endDate, period, homeFloor, site, sectorIds  } = props;

        const { data } = await this.client.post(
            `/collaborators/${userId}/hierarchy`,
            { startDate: startDate, endDate: endDate, sectorId: sectorId, 
                period: period.map((period) => Number(period.id)), 
                homeFloor: homeFloor.map((homeFloor) => Number(homeFloor.id)), 
                site: site.map((site) => Number(site.id)), sectorIds: sectorIds }
        );

        return data;
    }
}
