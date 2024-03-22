
export type ColorScheme = 'dark' | 'light';

export interface Settings {
  readonly credentials: {
    readonly userName: string,
    readonly password: string
  };

  readonly colorScheme: ColorScheme;
}
