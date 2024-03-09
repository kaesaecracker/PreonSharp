import React from "react";
import {Accordion, AccordionDetails, AccordionSummary} from "@mui/material";

export default function RunningQueryView(props: { query: string }) {
  const [expanded, setExpanded] = React.useState(true);

  return <Accordion
    expanded={expanded}
    onChange={(event, newExpanded) => setExpanded(newExpanded)}>
    <AccordionSummary>
      <p>result for query: {props.query} </p>
    </AccordionSummary>
    <AccordionDetails>
      running or cancelled
    </AccordionDetails>
  </Accordion>;
}
