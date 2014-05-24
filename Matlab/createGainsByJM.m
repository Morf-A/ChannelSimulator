function c = createGainsByJM(profile, sinusoidsNumber)
    sinusoidsNumber = sinusoidsNumber+1;
    rays = length(profile);
    c = zeros(2, sinusoidsNumber, rays);
    
    for z=1:rays
        for n=1:sinusoidsNumber-1
            c(1,n,z) = 2*profile(z)/(sqrt(sinusoidsNumber-0.5))*sin((pi*n)/(sinusoidsNumber-1));
        end
        
        for n=1:sinusoidsNumber-1
            c(2,n,z) = 2*profile(z)/(sqrt(sinusoidsNumber-0.5))*cos((pi*n)/(sinusoidsNumber-1));
        end
        
        for i = 1:2
            c(i,sinusoidsNumber,z) = 2*profile(z)/(sqrt(sinusoidsNumber-0.5));
        end
    end
end

