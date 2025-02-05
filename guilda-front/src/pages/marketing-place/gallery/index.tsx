import { ImageGallery } from "src/components/data-display/image-gallery/image-gallery";
import { getLayout } from "src/utils";

export default function Gallery() {
    return <ImageGallery />;
}

Gallery.getLayout = getLayout("private");
