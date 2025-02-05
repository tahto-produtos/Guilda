import * as React from "react";

interface IProps {
    width: number;
    height: number;
    color?: string;
}

export function ExpandIcon(props: IProps) {
    const { height, width, color } = props;

    return (
        <svg height={height} width={width} fill={color} viewBox="0 0 18 12">
            <path
                d="M0 12H18V10H0V12ZM0 7H18V5H0V7ZM0 0V2H18V0H0Z"
                fill={color}
            />
        </svg>
    );
}
