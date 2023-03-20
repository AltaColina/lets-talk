import { Suspense } from "react";
import { wrapPromise } from "../Callbacks/wrap-promise";
import { hubClient } from "../Services/hub-client";
import { Lobby } from "./Lobby";
import { Top } from "./Top";
import { Footer } from "./Footer";
import { Loading } from "./Loading";
import { MenuContextProvider } from "../Context/Menu";
import { Box } from "@mui/material";

export const Home = () => {
  const getUserRooms = wrapPromise(hubClient.getUserRooms());
  return (
    <Suspense fallback={<Loading />}>
      <MenuContextProvider>
        <Box sx={{ display: "flex" }}>
          <Top />
          <Lobby getUserRooms={getUserRooms} />
          <Footer />
        </Box>
      </MenuContextProvider>
    </Suspense>
  );
}