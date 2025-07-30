let interop = {
    unwrapObject: (object) => object,
    subscribeToEvent: (object, type, listener) => object.addEventListener(type, listener),

    item: (object, index) => object.item(index),
    contains: (object, string) => object.contains(string),

    createObjectStore: (object, string) => object.createObjectStore(string),

    transaction: (object, string, mode) => object.transaction(string, mode),
    objectStore: (object, string) => object.objectStore(string),

    add: (object, key, value) => object.add(key, value),
    get: (object, key) => object.get(key),
    count: (object) => object.count()
};

export { interop };