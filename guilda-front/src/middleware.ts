import { NextRequest, NextResponse } from "next/server";
import { jwtTokenKey, publicRoutes } from "./constants";
import { BASE_URL } from "./constants/environment-variable.constants";

export async function middleware(request: NextRequest) {
  const currentPath = request.nextUrl.pathname;
  const isCurrentPathProtected = !publicRoutes.includes(currentPath);

  if (isCurrentPathProtected) {
    const currentUserToken = request.cookies.get(jwtTokenKey);

    if (!currentUserToken) {
      return NextResponse.redirect(new URL(`${BASE_URL}/login`, request.url));
    }
    if (currentUserToken) {
      const isFirstLogin =
        request.cookies.get("firstLogin")?.value == "true" ? true : false;

      if (isFirstLogin && !currentPath.includes("/reset-password")) {
        return NextResponse.redirect(
          new URL(`${BASE_URL}/reset-password`, request.url)
        );
      }
    }
  }
}

export const config = {
  matcher: ["/((?!api|static|.\\..|_next).*)", { source: "/" }],
};
