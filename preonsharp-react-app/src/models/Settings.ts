export type ColorScheme = 'dark' | 'light';

export type Credentials = {
  readonly userName: string,
  readonly password: string
}

export type Settings = {
  readonly credentials: Credentials;
  readonly colorScheme: ColorScheme;
}

export function getDefaultSettings(): Settings {
  return {
    colorScheme: 'dark',
    credentials: {
      userName: '',
      password: ''
    }
  };
}
