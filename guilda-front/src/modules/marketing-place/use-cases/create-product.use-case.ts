import { guildaApiClient } from "src/services";

export interface CreateProductUseCaseProps {
    image?: File | null;
    description: string;
    code?: string;
    price: number;
    type: "PHYSICAL" | "DIGITAL";
    vouchers?: string[];
    status: string;
    comercialName: string;
    publicationDate: string;
    expirationDate: string | null;
    validityDate: string | null;
    // highlight: number;
    visibilitys?: Array<{ type: string; value: string }>;
    quantity?: number;
    uploadId?: string[] | null;
    stockId: number;
    saleLimit?: number;
    categoryId?: number | null;
    supplierId?: number | null;

    productDetailId?: number;
    sizeId?: number;
    typeId?: number;
    detailId?: number;
    groupId?: number;
    colorId?: number;
}

export class CreateProductUseCase {
    private client = guildaApiClient;

    async handle(props: CreateProductUseCaseProps) {
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
            visibilitys,
            quantity,
            uploadId,
            stockId,
            saleLimit,
            categoryId,
            supplierId,

            detailId,
            groupId,
            productDetailId,
            sizeId,
            typeId,
            colorId
        } = props;

        console.log("Aq");
        console.log(quantity);

        const form = new FormData();
        code && form.append("code", code);
        form.append("description", description);
        if (quantity !== undefined && quantity !== null) {
            form.append("quantity", quantity.toString());
        }
        saleLimit && form.append("saleLimit", saleLimit.toString());
        form.append("price", price.toString());

        detailId && form.append("detailId", detailId.toString());
        groupId && form.append("groupId", groupId.toString());
        productDetailId &&
            form.append("productDetailId", productDetailId.toString());
        sizeId && form.append("sizeId", sizeId.toString());
        typeId && form.append("typeId", typeId.toString());

        form.append("type", type.toString());
        form.append("status", status);
        form.append("comercialName", comercialName);
        form.append("publicationDate", publicationDate);
        expirationDate && form.append("expirationDate", expirationDate);
        validityDate && form.append("validityDate", validityDate);
        // form.append("highlight", highlight.toString());
        form.append("stockId", stockId.toString());
        categoryId && form.append("categoryId", categoryId.toString());
        supplierId && form.append("supplierId", supplierId.toString());
        colorId && form.append("colorId", colorId.toString());
        if (visibilitys) {
            for (let i = 0; i < visibilitys?.length; i++) {
                form.append(`visibilitys[${i}][type]`, visibilitys[i].type);
                form.append(`visibilitys[${i}][value]`, visibilitys[i].value);
            }
        }
        if (vouchers) {
            for (let i = 0; i < vouchers?.length; i++) {
                form.append("vouchers", vouchers[i].toString());
            }
        }

        image && !uploadId && form.append("images", image);

        if (uploadId) {
            for (let i = 0; i < uploadId?.length; i++) {
                form.append("uploadIds", uploadId[i]);
            }
        }

        const { data } = await this.client.post<FormData>("/product/v2", form);

        return data;
    }
}
