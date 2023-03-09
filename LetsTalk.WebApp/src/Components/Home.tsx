import { CircularProgress } from "@mui/material";
import { Suspense } from "react";
import { wrapPromise } from "../Callbacks/wrap-promise";
import { hubClient } from "../Services/hub-client";
import { Lobby } from "./Lobby";

export const Home = () => {
  const getUserRooms = wrapPromise(hubClient.getUserRooms());
  return (
    <Suspense fallback={<CircularProgress />}>
      <Lobby getUserRooms={getUserRooms} />
    </Suspense>
  );
}