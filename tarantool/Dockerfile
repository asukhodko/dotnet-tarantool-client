FROM tarantool/tarantool:1.7
WORKDIR /app
COPY . .
ENTRYPOINT ["tarantool", "/app/app.init.lua"]
