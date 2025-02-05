import { guildaApiClient } from "src/services";

export interface UpdateProductUseCaseProps {
    image?: File | null;
    description: string;
    code?: string;
    price: number;
    type: "PHYSICAL" | "DIGITAL";
    vouchers?: string[];
    status: string;
    comercialName: string;
    publicationDate: string;
    expirationDate?: string | null;
    validityDate: string | null;
    // highlight: number;
    id: string;
    visibilitys?: Array<{ type: string; value: string }>;
    uploadId?: string[] | null;
    saleLimit: number;
}

export class UpdateProductUseCase {
    private client = guildaApiClient;

    async handle(props: UpdateProductUseCaseProps) {
        const {
            code,
            description,
            image,
            price,
            type,
            vouchers,
            comercialName,
            expirationDate,
            // highlight,
            publicationDate,
            validityDate,
            status,
            id,
            visibilitys,
            uploadId,
            saleLimit,
        } = props;

        const form = new FormData();
        code && form.append("code", code);
        form.append("description", description);
        form.append("price", price.toString());
        form.append("saleLimit", saleLimit ? saleLimit?.toString() : "0");
        form.append("type", type.toString());
        form.append("status", status);
        form.append("comercialName", comercialName);
        form.append("publicationDate", publicationDate);
        expirationDate && form.append("expirationDate", expirationDate);
        validityDate && form.append("validityDate", validityDate);
        // form.append("highlight", highlight.toString());
        if (vouchers) {
            for (let i = 0; i < vouchers?.length; i++) {
                form.append("vouchers", vouchers[i].toString());
            }
        }
        if (visibilitys) {
            for (let i = 0; i < visibilitys?.length; i++) {
                form.append(`visibilitys[${i}][type]`, visibilitys[i].type);
                form.append(`visibilitys[${i}][value]`, visibilitys[i].value);
            }
        }

        image && form.append("images", image);

        if (uploadId) {
            for (let i = 0; i < uploadId?.length; i++) {
                form.append("uploadIds", uploadId[i]);
            }
        }

        const { data } = await this.client.put<FormData>(
            `/product/${id}`,
            form
        );

        return data;
    }
}
