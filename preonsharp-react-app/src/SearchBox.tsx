import {ChangeEvent, KeyboardEvent, useState} from "react";
import {Button, TextField} from "@mui/material";

import './SearchBox.css';
import SearchIcon from '@mui/icons-material/Search';

function SearchBox(props: { onSearch: (queryText: string) => void }) {
  const [inputValue, setInputValue] = useState('');

  const onKeyDown = (event: KeyboardEvent) => {
    if (event.key === 'Enter') {
      props.onSearch(inputValue);
    }
  };

  return <div className="SearchBox">
    <TextField
      label="request"
      type='text'
      value={inputValue}
      onChange={(event: ChangeEvent<HTMLInputElement>) => setInputValue(event.target.value)}
      className="query-input"
      onKeyDown={onKeyDown}
    />
    <Button onClick={() => props.onSearch(inputValue)} variant='outlined'>
      <SearchIcon/>
    </Button>
  </div>;
}

export default SearchBox;
