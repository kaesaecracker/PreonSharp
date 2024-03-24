import {Credentials} from "./models/Settings.ts";

export async function fetchData<T>({url, credentials}: { url: URL, credentials: Credentials }): Promise<T> {
  if (!credentials.userName || !credentials.userName)
    throw new Error('no user credentials provided, check settings');

  const response = await fetch(url, {
    headers: new Headers({
      "Authorization": `Basic ${btoa(`${credentials.userName}:${credentials.password}`)}`
    }),
  });

  if (!response.ok)
    throw new Error('server did not respond with success code');

  return await response.json() as T;
}
