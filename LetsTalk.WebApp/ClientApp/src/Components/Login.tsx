import LockIcon from '@mui/icons-material/Lock';
import { Avatar, Paper } from "@mui/material";
import { Box } from "@mui/system";
import { useEffect, useState } from "react";
import { Navigate } from 'react-router-dom';
import { UserProfile } from '../Security/user-profile';
import { hubClient } from '../Services/hub-client';

export const Login = () => {
  const [userProfile, setUserProfile] = useState(UserProfile.guestProfile);
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  useEffect(() => {
    (async () => {
      const profile = await UserProfile.currentProfile;
      if(profile.isLoggedIn) {
          if(!hubClient.isConnected && !hubClient.isConnecting) {
          await hubClient.connect();
        }
        setIsLoggedIn(true);
      }
      setUserProfile(profile);
    })();
  }, []);

  return (
    <Paper
      sx={{
        minHeight: '25em',
        maxHeight: '50em',
        minWidth: '20em',
        maxWidth: '25em',
        margin: '20px auto'
      }}
      elevation={10}>
      <Box
        component="form"
        sx={{
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'center',
          alignItems: 'center',
          p: 3,
          gap: 1
        }}
        noValidate
        autoComplete="off">
        {isLoggedIn && (<Navigate to="/" replace={true} />)}
        <Avatar sx={{ backgroundColor: 'green' }}><LockIcon /></Avatar>
        <h2>Sign In</h2>
        {
          userProfile.isLoggedIn
            ? <a href={userProfile.logoutUrl} rel="noopener noreferrer">logout</a>
            : <a href={'/bff/login'} rel="noopener noreferrer">login</a>
        }
        <div>Hello, {userProfile.displayName || 'guest'}!</div>
      </Box>
    </Paper>
  );
}