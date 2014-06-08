function ach = getFrequencyResponse(channel, F, time, Ts)
 K=size(channel,3);
 rays=size(channel,1);
ach = zeros(F,K);
    for k=1:K
        for frec=1:F
            for z=1:rays
                h = channel(z,1,k)+1j*channel(z,2,k);
                ach(frec,k)=ach(frec,k)+h*exp(-1j*2*pi*15000*(frec-1)*time(z)*10^-9);
            end
        end
    end
[X,Y] = meshgrid(0:Ts:Ts*(100-1),0:15000:15000*(100-1));
surf(X,Y,ach(1:100,1:100).*conj(ach(1:100,1:100)));
ylabel('частота, √ц', 'FontSize', 14);
xlabel('модельное врем€, с', 'FontSize', 14);
zlabel('|H|^2', 'FontSize', 14);
title('„астотный отклик канала', 'FontSize', 14);
%plot(ach(:,1).*conj(ach(:,1))) ;
end

