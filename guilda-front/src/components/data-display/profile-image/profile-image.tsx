import { Avatar, CardMedia, Stack, useTheme } from "@mui/material";

interface ProfileImageProps {
    width: string;
    height: string;
    name?: string;
    image?: string;
    color?: string;
    marginTop?: string;
}

export function ProfileImage(props: ProfileImageProps) {
    const { height, width, name, image, color, marginTop } = props;
    const theme = useTheme();

    return (
        <Stack>
            {image ? (
                <CardMedia
                    component="img"
                    image={image}
                    sx={{
                        width,
                        height,
                        marginTop,
                        borderRadius: 999,
                        objectFit: "cover",
                    }}
                />
            ) : name ? (
                <Avatar
                    sx={{
                        width,
                        height,
                        marginTop,
                        fontSize: 16,
                        bgcolor: color ? color : theme.palette.secondary.main,
                    }}
                >
                    {name.charAt(0)}
                    {name.split(" ")[1]?.charAt(0)}
                </Avatar>
            ) : (
                <Avatar
                    sx={{
                        width,
                        height,
                        marginTop,
                        fontSize: 16,
                        bgcolor: color ? color : theme.palette.secondary.main,
                    }}
                />
            )}
        </Stack>
    );
}
