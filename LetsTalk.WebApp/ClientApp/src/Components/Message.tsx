import { Breadcrumbs, Grid, Paper } from "@mui/material";
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
        <Breadcrumbs>
          Tiago
          Test
        </Breadcrumbs>
        <Grid container spacing={2} textAlign={"left"} fontStyle={"italic"} fontSize={'14px'}>
          <Grid item>
            <span>{message.userName}</span>
          </Grid>
          <Grid item textAlign={"right"}>
            <span>{new Date(message.timestamp).toLocaleString()}</span>
          </Grid>
        </Grid>
        {renderer(message.content)}
      </Grid>
    </Paper>
  );
}