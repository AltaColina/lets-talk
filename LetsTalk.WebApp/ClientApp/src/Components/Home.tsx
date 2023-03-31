import { Box } from "@mui/material";
import { Suspense } from "react";
import wrapPromise from "../Callbacks/wrap-promise";
import { MenuContextProvider } from "../Context/Menu";
import { hubClient } from "../Services/hub-client";
import { Footer } from "./Footer";
import { Loading } from "./Loading";
import { Lobby } from "./Lobby";
import { Top } from "./Top";

export const Home = () => {
  const getUserRooms = wrapPromise(hubClient.getRoomsWithUser());
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