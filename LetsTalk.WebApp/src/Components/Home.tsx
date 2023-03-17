import { CircularProgress } from "@mui/material";
import { Suspense } from "react";
import { wrapPromise } from "../Callbacks/wrap-promise";
import { hubClient } from "../Services/hub-client";
import { Lobby } from "./Lobby";
import { Top } from "./Top";
import { Footer } from "./Footer";

export const Home = () => {
  const getUserRooms = wrapPromise(hubClient.getUserRooms());
  return (
    <Suspense fallback={<CircularProgress />}>
      <Top />
      <Lobby getUserRooms={getUserRooms} />
      <Footer />
    </Suspense>
  );
}