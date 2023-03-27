import { Grid, Paper } from "@mui/material";
import { ContentMessage } from "../Messaging/content-message";

const getContentRenderer = (contentType: string): (content: string) => JSX.Element => {
  switch (contentType) {
    case 'text/plain':
      return (content: string) => (<div>{Buffer.from(content, 'base64').toString()}</div>);
    default:
      return (content: string) => (<div>{content}</div>);
  }
}

export const Message = ({ message }: { message: ContentMessage }) => {
  const renderer = getContentRenderer(message.contentType);
  return (
    <Paper>
      <Grid container direction={"column"} margin={1} padding={1} textAlign={"left"}>
        <Grid container spacing={2} textAlign={"left"} fontStyle={"italic"} fontSize={'14px'}>
          <Grid item>
            {message.userName}
          </Grid>
          <Grid item textAlign={"right"}>
            {new Date(message.timestamp).toLocaleString()}
          </Grid>
          <Grid item textAlign={"right"}>
            {message.id}
          </Grid>
        </Grid>
        {renderer(message.content)}
      </Grid>
    </Paper>
  );
}