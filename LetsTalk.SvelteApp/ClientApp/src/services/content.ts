import { Buffer } from 'buffer';

export const encodeText = function () {
    const encoder = new TextEncoder();
    return (content: string | undefined) => Buffer.from(encoder.encode(content)).toString('base64');
}();

export function decodeText(content: string) {
    return Buffer.from(content, 'base64').toString();
}