export interface LoginDto {
    email: string;
    password: string;
}

export interface RegisterDto {
    email: string;
    password: string;
}

export interface AuthResponseDto {
    token: string;
    email: string;
    role: string;
}

export interface CurrentUser {
    email: string;
    role: string;
}
