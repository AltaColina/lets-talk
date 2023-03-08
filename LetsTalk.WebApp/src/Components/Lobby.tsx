import { CircularProgress } from "@mui/material";
import { Suspense, useEffect, useState } from "react";
import { Chat } from "../Chats/chat";
import { GetUserChatsResponse } from "../Chats/getUserChatsResponse";
import { Authentication } from "../Security/authentitcation";
import { hubClient } from "../Services/hub-client";
import { messenger } from "../Services/messenger";
import { wrapPromise } from "../Services/wrap-promise";

const InnerLobby = ({ resource }: { resource: () => GetUserChatsResponse } ) => {
  const response = resource();
  return (
    <div>{
      response.chats.map(chat => (
        <div key={chat.id}>
          {chat.name}
        </div>
      ))
    }
    </div>
  );
}

export const Lobby = () => {
  const resource = wrapPromise(hubClient.getUserChats());
  return (
    <Suspense fallback={<CircularProgress />}>
      <InnerLobby resource={resource} />
    </Suspense>
  );
}