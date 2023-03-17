import { Avatar, Paper, TextField } from "@mui/material";
import Button from "@mui/material/Button";
import { Box } from "@mui/system";
import React, { useState } from "react";
import { Navigate } from 'react-router-dom';
import { httpClient } from "../Services/http-client";
import { hubClient } from "../Services/hub-client";
import { messenger } from "../Services/messenger";
import LockIcon from '@mui/icons-material/Lock';

const areValidCredentials = (username?: string, password?: string): boolean => {
  if (!username || !password)
    return false;
  if (username.length === 0 || password.length === 0)
    return false;
  return true;
}

export const Login = () => {
  const [values, setValues] = useState({
    isValid: false,
    isLoggedIn: false,
    username: undefined as string | undefined,
    password: undefined as string | undefined,
  });
  

  const onUsernameChanged = (e: React.ChangeEvent<HTMLInputElement>) => {
    setValues({
      ...values,
      username: e.target.value,
      isValid: areValidCredentials(e.target.value, values.password)
    });
  }

  const onPasswordChanged = (e: React.ChangeEvent<HTMLInputElement>) => {
    setValues({
      ...values,
      password: e.target.value,
      isValid: areValidCredentials(values.username, e.target.value)
    });
  }

  const onLoginClicked = async (): Promise<void> => {
    const auth = await httpClient.auth.login(values.username!, values.password!);
    await hubClient.connect('/hubs/letstalk', messenger, () => auth.accessToken);
    setValues({
      ...values,
      isLoggedIn: true
    });
  }

  const onPasswordKeyDown = async (e: React.KeyboardEvent<HTMLInputElement>) => {
    if(e.key === 'Enter') {
        e.preventDefault();
        await onLoginClicked();
    }
}

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
          {values.isLoggedIn && (<Navigate to="/" replace={true} />)}
          <Avatar sx={{ backgroundColor: 'green' }}><LockIcon/></Avatar>
          <h2>Sign In</h2>
          <TextField
            required
            fullWidth
            label="Username"
            type='text'
            onChange={onUsernameChanged} />
          <TextField
            // sx={{ '& label.Mui-focused': { color: 'green' } }}
            // sx={{ '& .MuiOutlinedInput-root': { '&.Mui-focused fieldset': { borderColor: 'green' } } }}
            // sx={{ '& .MuiOutlinedInput-root': { '& input:invalid + fieldset': { borderColor: 'red' } } }}
            required
            fullWidth
            label="Password"
            type="password"
            onChange={onPasswordChanged}
            onKeyDown={onPasswordKeyDown} />
          <Button
            fullWidth
            variant="contained"
            onClick={onLoginClicked}
            disabled={!values.isValid}>
            Login
          </Button>
        </Box>
      </Paper>
  );
}