function result = processSignal(signal, channel, tau, Td)
    rays =size(channel,1);
    %K=size(channel,3);
    result = zeros(2000,1);
    channelK = 1;
    step = 100;
    for k=100:2000  
        for z = 1:rays
            Hz =  channel(z,1,channelK)+1j*channel(z,2,channelK);
            
            result(k) = result(k)+Hz*signal(k-round((tau(z)*10^-9)/Td));
        end
        if mod(k-1,step)==0
            channelK = channelK+1;
        end
    end
end

