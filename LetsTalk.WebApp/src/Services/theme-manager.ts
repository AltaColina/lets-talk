import { createTheme, Theme } from "@mui/material";

const themes = {
    light: createTheme({
        palette: {
            mode: 'light'
        }
    }),
    dark: createTheme({
        palette: {
            mode: 'dark'
        }
    })
};

export type ThemeName = keyof typeof themes;

class ThemeManager {
    private _themeName: ThemeName = 'light';
    private _theme: Theme = themes.light;

    public get themeName(): ThemeName {
        return this._themeName;
    }
    public set themeName(value: ThemeName) {
        if (value && value !== this._themeName) {
            const theme = themes[this._themeName];
            if (theme) {
                this._themeName = value;
                this._theme = theme;
                document.dispatchEvent(new CustomEvent<Theme>('ThemeChanged', { detail: this._theme }));
            }
            else {
                console.warn(`Theme '${value}' does not exist`);
            }

        }
    }

    public get theme(): Theme {
        return this._theme;
    }

    public addThemeChangedListener(handler: (e: CustomEvent<Theme>) => any): () => any {
        document.addEventListener<any>('ThemeChanged', handler);
        return () => document.removeEventListener<any>('ThemeChanged', handler);
    }
}

export const themeManager = new ThemeManager();

themeManager.addThemeChangedListener(e => e.detail);