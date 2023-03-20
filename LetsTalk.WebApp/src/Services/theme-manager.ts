import { createTheme, Theme } from "@mui/material";
import { amber, deepOrange, purple, red } from "@mui/material/colors";
import { messenger } from "./messenger";

const themes = {
    light: createTheme({
        palette: {
            mode: 'light'
        }
    }),
    dark: createTheme({
        palette: {
            mode: 'dark',
            primary: {
                main: amber[300]
            },
            background: {
                default: deepOrange[900],
                paper: deepOrange[900]
            },
            text: {
                primary: purple[900],
                secondary: red[500]
            }
        }
    })
};

// const getDesignTokens = (mode: PaletteMode) => ({
//   palette: {
//     mode,
//     primary: {
//       ...amber,
//       ...(mode === "dark" && {
//         main: amber[300]
//       })
//     },
//     ...(mode === "dark" && {
//       background: {
//         default: deepOrange[900],
//         paper: deepOrange[900]
//       }
//     }),
//     text: {
//       ...(mode === "light"
//         ? {
//           primary: purple[900],
//           secondary: purple[800]
//         }
//         : {
//           primary: green[500],
//           secondary: green[500]
//         })
//     }
//   }
// });

// const customModeTheme = createTheme(getDesignTokens("dark"));

export type ThemeName = keyof typeof themes;

class ThemeManager {
    private _themeName: ThemeName = 'light';
    private _theme: Theme = themes.light;

    public get themeName(): ThemeName {
        return this._themeName;
    }
    public set themeName(value: ThemeName) {
        if (value && value !== this._themeName) {
            const theme = themes[value];
            if (theme) {
                this._themeName = value;
                this._theme = theme;
                messenger.send('ThemeChanged', this._theme);
            }
            else {
                console.warn(`Theme '${value}' does not exist`);
            }

        }
    }

    public get theme(): Theme {
        return this._theme;
    }
}

export const themeManager = new ThemeManager();