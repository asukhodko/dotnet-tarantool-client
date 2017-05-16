#!/usr/bin/env tarantool

box.cfg {}

box.schema.user.create('mytestuser', { password = 'mytestpass' })
box.schema.user.grant('mytestuser', 'read,write', 'space', '_schema')
box.schema.user.grant('mytestuser', 'read,write', 'space', '_space')
box.schema.user.grant('mytestuser', 'read,write', 'space', '_index')
box.schema.user.grant('mytestuser', 'read', 'space', '_priv')
box.schema.user.grant('mytestuser', 'execute', 'universe')

function some_function()
    return "ok"
end

box.schema.create_space('test', {user = 'mytestuser'})
box.space.test:create_index('primary', {type = 'tree', parts = {1, 'unsigned'}})

box.schema.func.create('some_function')
box.schema.user.grant('mytestuser', 'execute', 'function', 'some_function')

s = box.space.test
s:insert({1, 'Roxette'})
s:insert({2, 'Scorpions', 2015})
s:insert({3, 'Ace of Base', 1993})
