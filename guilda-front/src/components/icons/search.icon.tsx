import * as React from "react";

interface IProps {
    width: number;
    height: number;
    color?: string;
}

export function SearchIcon(props: IProps) {
    const { height, width, color } = props;

    return (
        <svg height={height} width={width} viewBox="0 0 19 18" fill="none">
            <path
                d="M13.2192 11H12.4031L12.1139 10.73C13.1262 9.59 13.7357 8.11 13.7357 6.5C13.7357 2.91 10.7296 0 7.02116 0C3.31268 0 0.306641 2.91 0.306641 6.5C0.306641 10.09 3.31268 13 7.02116 13C8.6843 13 10.2131 12.41 11.3908 11.43L11.6697 11.71V12.5L16.8347 17.49L18.3739 16L13.2192 11ZM7.02116 11C4.44898 11 2.37265 8.99 2.37265 6.5C2.37265 4.01 4.44898 2 7.02116 2C9.59334 2 11.6697 4.01 11.6697 6.5C11.6697 8.99 9.59334 11 7.02116 11Z"
                fill={color}
            />
        </svg>
    );
}
