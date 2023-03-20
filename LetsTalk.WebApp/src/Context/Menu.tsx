import { createContext, Dispatch, ReactNode, SetStateAction, useContext, useState } from "react";

export type MenuContextType = {
    openMenu: boolean;
    // setOpen: Dispatch<SetStateAction<boolean>>;
    toggleMenu: () => void;
};

export const MenuContext = createContext<MenuContextType | null>(null);

type Props = {
    children: ReactNode;
};

export const MenuContextProvider = ({ children }: Props) => {
    const [ openMenu, setOpen ] = useState(true);
    const toggleMenu = () => {
        setOpen(!openMenu)
    };
    return(
        <MenuContext.Provider value={{ openMenu, toggleMenu }}>
            { children }
        </MenuContext.Provider>
    );
};

export const useMenuContext = () => {
    const menuContext = useContext(MenuContext);
    if (!menuContext) throw new Error('Context most be within provider!');
    return menuContext;
}