import {useState} from "react";

export default function useStoredState(storageKey: string, initialState: string): [string, ((newState: string) => void)] {
  const [state, setState] = useState<string>(() => localStorage.getItem(storageKey) || initialState);

  const setSavedState = (newState: string) => {
    localStorage.setItem(storageKey, newState);
    setState(newState);
  };

  return [state, setSavedState];
}
