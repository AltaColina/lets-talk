import { createTheme, Theme } from "@mui/material";

const themes: Map<string, Theme> = new Map<string, Theme>(
    [
        ['light', createTheme({
            palette: {
                mode: 'light'
            }
        })],
        ['dark', createTheme({
            palette: {
                mode: 'dark'
            }
        })]
    ]
);

class ThemeManager {
    private _mode: string = 'light';
    private _theme: Theme = themes.get(this._mode)!;
    public get themeMode(): string {
        return this._theme.palette.mode
    }
    public set themeMode(mode: string) {
        const theme = themes.get(mode);
        if (theme) {
            this._theme = theme;
            document.dispatchEvent(new CustomEvent('ThemeChanged', { detail: this._mode }));
        } else {
            console.warn(`Theme ${mode} does not exist.`);
        }
    }
    public theme(mode: string): Theme {
        return themes.get(mode)!;
    }

    public addThemeChangedListener(handler: (e: CustomEvent<string>) => any): () => any {
        document.addEventListener<any>('ThemeChanged', handler);
        return () => document.removeEventListener<any>('ThemeChanged', handler);
    }
}

export const themeManager = new ThemeManager();

themeManager.addThemeChangedListener(e => e.detail);