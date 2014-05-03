function p = createPhasesByMEDS(sinusoidsNumber)

p=zeros(2,sinusoidsNumber+1);
    for i=1:2
        for n=1:sinusoidsNumber
            p(i,n) = 2*pi*rand(1)-pi;
        end
        sinusoidsNumber = sinusoidsNumber+1;
    end
end

