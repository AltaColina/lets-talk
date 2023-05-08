type PromiseState = 'pending' | 'success' | 'error';

export default function wrapPromise<T>(promise: PromiseLike<T>): () => T {
    let status: PromiseState = 'pending';
    let response: T;
    const suspender = promise.then(
        res => {
            status = 'success'
            response = res
        },
        err => {
            status = 'success'
            response = err
        });
    return () => {
        switch(status) {
            case 'pending':
                throw suspender;
            case 'error':
                throw response;
            default:
                return response;
        }
    }
}