import { ReactElement } from "react";
import { PublicLayout } from "../layouts";
import { PrivateLayout } from "../layouts/private";

const availableLayouts = {
  public: PublicLayout,
  private: PrivateLayout,
};

export function getLayout(layout: keyof typeof availableLayouts) {
  const Layout = availableLayouts[layout];
  return (page: ReactElement) => <Layout>{page}</Layout>;
}
