import * as React from "react";

interface IProps {
    width: number;
    height: number;
    color?: string;
}

export function HomeIcon(props: IProps) {
    const { height, width, color } = props;

    return (
        <svg
            height={height}
            width={width}
            fill={color}
            viewBox="0 0 20 17"
            xmlns="http://www.w3.org/2000/svg"
        >
            <path
                d="M10 2.69L15 7.19V15H13V9H7V15H5V7.19L10 2.69ZM10 0L0 9H3V17H9V11H11V17H17V9H20L10 0Z"
                fill={color}
            />
        </svg>
    );
}
